# temporal-examples

## Purpose of this Repo

This is to show several features of Temporal in a way that is easy to understand and run. All you
need is Docker as everything will run in Docker containers.

It will run a [Swagger page](http://localhost:8080/swagger/index.html) that allows you to start
workflows and send signals to paused workflows via a REST API.

## Running the project - you'll need Docker

Make sure your terminal is in the temporal-examples folder and then run `docker compose up`. This
will start all services. If you ever need to debug the Temporal workflows you can debug via a Docker
container by installing the "Container Tools" extension in VS Code. There is also an option to do
the debugging through Visual Studio tooling as well.

This set of Docker containers actually installs the Temporal CLI to an isolated. If you want to
execute commands using it you should either exec into it or access exec in Docker Desktop. The
container is named `temporal-cli`. If you want examples of this go to the `temporal-examples/scripts`
folder. There is also the documentation [here](https://docs.temporal.io/cli#command-set)

## Examples

We have several different examples of what Temporal can do in the Workflows files which are in the
Workflows project.

**Examples.workflow.cs** - shows a default Temporal workflow with a generic activity

**ExampleWithChildren.workflow.cs** - shows you how you can compose workflows together. This creates
1-10 child workflows and waits for them to finish.

**WaitingSignal.workflow.cs** - shows how you can pause a workflow indefinitely or with a timeout to
wait for a "signal". This signal in our example is caused by a REST API. While paused the workflow
takes up very few resources. The SignalController inside the rest-controller project is where we send
the signal. This also shows how you can search the Temporal Server for specific workflows. In this
case we are using the Workflow ID.

## Monitoring and Metrics

The docker compose file contains [Prometheus](https://prometheus.io/) and
[Grafana](https://grafana.com/) services. Both the Temporal service and all worker services expose
metrics to Prometheus, which are collected based on the settings in `prometheus.yml`. Prometheus can
be manually added to Grafana as a data source.

The Prometheus UI can be accessed at [query page](http://localhost:9090/query), and details of the
different scraped metrics endpoints can be seen at [targets page](http://localhost:9090/targets).

The Grafana UI can be accessed at [Grafana login](http://localhost:3000/login) - Login details are
admin/admin.

## License

See License.md
