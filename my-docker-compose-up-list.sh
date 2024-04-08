#!/bin/sh

docker rm myrecall-recall-listapi-1
docker image rm recall-listapi

docker compose up
