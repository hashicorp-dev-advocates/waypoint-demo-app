package main

import (
	"bytes"
	"crypto/tls"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"sync"
	"time"

	"github.com/brianvoe/gofakeit/v6"
	"github.com/kr/pretty"
	"github.com/nicholasjackson/env"

	"github.com/go-chi/chi"
	"github.com/go-chi/chi/middleware"
	"github.com/hashicorp/go-hclog"
)

var log hclog.Logger
var bind = env.String("BIND_ADDR", false, ":3000", "URL for the payments service")
var payments = env.String("PAYMENTS", false, "http://localhost:3001/Payments", "URL for the payments service")
var defaultClient *http.Client

func main() {
	env.Parse()

	log = hclog.New(&hclog.LoggerOptions{
		Level: hclog.Debug,
		Color: hclog.AutoColor,
	})

	defaultClient = &http.Client{
		Transport: &http.Transport{
			DisableKeepAlives: true,
			TLSClientConfig:   &tls.Config{InsecureSkipVerify: true},
		},
		Timeout: 30 * time.Second,
	}

	r := chi.NewRouter()

	r.Use(middleware.Logger)
	r.Post("/v1/pay", doPay)

	r.Get("/ready", healthHandler)
	r.Get("/health", healthHandler)

	log.Info("Starting server")

	http.ListenAndServe(*bind, r)
}

func healthHandler(w http.ResponseWriter, r *http.Request) {
	w.WriteHeader(http.StatusOK)
}

type PaymentRequest struct {
	//TransactionID string  `fake:"{regex:[a-z]{64}}"`
	Name        string  `fake:"{name}"`
	Type        string  `fake:"{creditcardtype}"`
	Number      string  `fake:"{creditcardnumber}"`
	Exp         string  `fake:"{creditcardexp}"`
	CVV         string  `fake:"{creditcardcvv}"`
	Amount      float64 `fake:"{price:1,1000}"`
	DateCreated time.Time
}

var requestCount = 0
var mutex sync.Mutex

func doPay(w http.ResponseWriter, r *http.Request) {
	pr := &PaymentRequest{}
	gofakeit.Struct(pr)
	pr.DateCreated = time.Now()

	// every 10th request add a double barreled name
	mutex.Lock()
	requestCount++

	// if request count is equal to max int reset
	if requestCount == int(^uint(0)>>1) {
		requestCount = 0
	}
	mutex.Unlock()

	if requestCount%3 == 0 {
		pr.Name = fmt.Sprintf("%s %s-%s %s", gofakeit.NamePrefix(), pr.Name, gofakeit.LastName(), gofakeit.NameSuffix())
		pr.CVV = fmt.Sprintf("9%s", pr.CVV)
	}

	d, _ := json.Marshal(pr)
	req, _ := http.NewRequest(http.MethodPost, *payments, bytes.NewBuffer(d))
	req.Header.Add("content-type", "application/json")

	log.Debug("Writing body", "body", pretty.Sprint(pr))

	resp, err := defaultClient.Do(req)
	if err != nil {
		log.Error("Unable to execute request", "error", err)
		http.Error(w, "unable to execute request", http.StatusInternalServerError)
		return
	}

	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusNotModified {
		body, _ := ioutil.ReadAll(resp.Body)
		log.Error("Expected status 200 or 201, got", "status", resp.StatusCode, "response", string(body))
	}

	w.WriteHeader(resp.StatusCode)
}
