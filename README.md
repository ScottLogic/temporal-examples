# temporal-examples

TODO - Add README for existing content

## Monitoring and Metrics

The docker compose file contains [Prometheus](https://prometheus.io/) and [Grafana](https://grafana.com/) services.
Both the Temporal service and all worker services expose metrics to Prometheus, which are collected based on the
settings in `prometheus.yml`. Prometheus can be manually added to Grafana as a data source.
