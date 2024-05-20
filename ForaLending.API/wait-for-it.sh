#!/usr/bin/env bash

# wait-for-it.sh - A script to wait for a service to be ready

set -e

TIMEOUT=60
QUIET=0
HOST="$1"
shift
PORT="$1"
shift
CMD="$@"

>&2 echo "Waiting for ${HOST}:${PORT} to be ready..."

for i in `seq $TIMEOUT` ; do
    if nc -z "$HOST" "$PORT" ; then
        >&2 echo "${HOST}:${PORT} is up - executing command"
        exec $CMD
    fi
    sleep 1
done

>&2 echo "${HOST}:${PORT} is not available after ${TIMEOUT} seconds"
exit 1
