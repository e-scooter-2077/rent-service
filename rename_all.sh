#!/bin/bash

if [[ $# != 1 ]] ; then
  >&2 echo "Usage: $0 MICROSERVICE_NAME"
  exit 1
fi

MICROSERVICE_NAME="$1"

find . -type d -path '*/.git' -prune -o \
  -type d -path "*/bin" -prune -o \
  -type d -path "*/obj" -prune -o \
  -not -name ".gitignore" \
  -not -name ".gitattributes" \
  -not -wholename "$0" \
  -not -wholename . \
  -print | sort -r | while read FILE_NAME
do
  PATTERN='s/___MICROSERVICE_NAME___/'"$MICROSERVICE_NAME"'/g'
  if [[ -f "$FILE_NAME" ]] ; then
    NEW_FILE_NAME=`echo "$FILE_NAME" | sed -e "$PATTERN"`
    mkdir -p `dirname "$NEW_FILE_NAME"`
    mv "$FILE_NAME" "$NEW_FILE_NAME"
    sed -i "$PATTERN" "$NEW_FILE_NAME"
  else
    rmdir "$FILE_NAME" &> /dev/null || true
  fi
done

rm "$0" # self-delete
