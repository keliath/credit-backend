#!/usr/bin/env bash
#   Use this script to test if a given TCP host/port are available

# Copied from https://github.com/vishnubob/wait-for-it (MIT License)

set -e

TIMEOUT=15
QUIET=0
HOST=""
PORT=""

while [[ $# -gt 0 ]]; do
    case "$1" in
        -t|--timeout)
            TIMEOUT="$2"
            shift 2
            ;;
        -q|--quiet)
            QUIET=1
            shift
            ;;
        --)
            shift
            break
            ;;
        *)
            if [[ -z "$HOST" ]]; then
                HOST="$1"
            elif [[ -z "$PORT" ]]; then
                PORT="$1"
            fi
            shift
            ;;
    esac
done

if [[ -z "$HOST" || -z "$PORT" ]]; then
    echo "Usage: $0 host:port [-t timeout] [-q] [-- command args]"
    exit 1
fi

for i in $(seq $TIMEOUT); do
    if nc -z "$HOST" "$PORT"; then
        [[ $QUIET -ne 1 ]] && echo "$HOST:$PORT is available after $i seconds"
        exec "$@"
        exit 0
    fi
    sleep 1
done

[[ $QUIET -ne 1 ]] && echo "Timeout after $TIMEOUT seconds waiting for $HOST:$PORT"
exit 1 