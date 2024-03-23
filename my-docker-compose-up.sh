#!/bin/sh

docker rm myrecall-recall-textitemapi-1
docker image rm recall-textitemapi

docker compose up
