# temporal-examples

TODO - Add README for existing content

## Monitoring and Metrics

The docker compose file contains [Prometheus](https://prometheus.io/) and [Grafana](https://grafana.com/) services.
Both the Temporal service and all worker services expose metrics to Prometheus, which are collected based on the
settings in `prometheus.yml`. Prometheus can be manually added to Grafana as a data source.

The Prometheus UI can be accessed at [http://localhost:9090/query], and details of the different scraped metrics
endpoints can be seen at [http://localhost:9090/targets].

The Grafana UI can be accessed at [http://localhost:3000/login] - Login details are admin/admin.
