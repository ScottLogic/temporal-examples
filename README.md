# temporal-examples

## Purpose of this Repo

This is to show several features of temporal in a way that is easy to understand and run. All you need is docker as everything will run in docker containers.

It will run a swagger page that allows you to start workflows using a rest api.

## Running the project - you'll need Docker

Make sure your terminal is in the temporal-examples folder and then run docker compose up. This should also run the temporal workers as well. If you ever need to debug the temporal workflows you can debug via a docker container by installing the "Container Tools" extension in VS Code. There is also an option to do the debugging through Visual Studio tooling as well.

This set of docker containers actually installs the temporal cli to a container. If you want to execute commands using it you should either exec into it or access exec in Docker Desktop. The container is named temporal-cli. If you want examples of this go to the temporal-examples/scripts folder.

## Monitoring and Metrics

The docker compose file contains [Prometheus](https://prometheus.io/) and [Grafana](https://grafana.com/) services.
Both the Temporal service and all worker services expose metrics to Prometheus, which are collected based on the
settings in `prometheus.yml`. Prometheus can be manually added to Grafana as a data source.

The Prometheus UI can be accessed at [query page](http://localhost:9090/query), and details of the different scraped metrics
endpoints can be seen at [targets page](http://localhost:9090/targets).

The Grafana UI can be accessed at [Grafana login](http://localhost:3000/login) - Login details are admin/admin.

## People to talk to about Temporal in Scott Logic

Silas Benson, Roger Stillito, Fanis Vlachos, James Strachan, Matt Griffin, Wayne Eastaugh, Dan Moorhouse, Yotaka Blowers, Matt Cadden, David Bell all worked in a large project involving data pipelines using Temporal.

## License

See License.md
