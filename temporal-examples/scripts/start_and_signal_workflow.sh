#!/bin/sh

WORKFLOW_ID="$(date +%s)-$RANDOM"

echo "Starting workflow with ID: $WORKFLOW_ID"
temporal workflow start --type WaitingSignalWorkflow --task-queue example --workflow-id "$WORKFLOW_ID"

sleep 2
read -p "Enter your name to signal the workflow: " SIGNAL_INPUT

# Send the signal to the workflow
temporal workflow signal --workflow-id "$WORKFLOW_ID" --name Continue --input "\"$SIGNAL_INPUT\""

echo "Signal sent to workflow $WORKFLOW_ID"


