#!/bin/sh

echo "Starting workflow of type ExampleWorkflow"
temporal workflow start --type ExampleWorkflow --task-queue example
sleep 2
# List all workflows
temporal workflow list