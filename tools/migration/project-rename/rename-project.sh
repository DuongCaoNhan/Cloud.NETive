#!/bin/bash
# Rename project (Linux/macOS)
OLD_NAME=$1; NEW_NAME=$2
find . -type f | xargs sed -i "s/$OLD_NAME/$NEW_NAME/g"
