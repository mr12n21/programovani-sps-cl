#!/bin/bash

echo "Soubor | Původní velikost (B) | Velikost tar.gz (B) | Úspora (%)"
echo "-------------------------------------------------------------"

for file in *.txt *.png; do
    [ -f "$file" ] || continue

    original_size=$(stat -c %s "$file")

    archive="${file}.tar.gz"
    tar -czf "$archive" "$file"

    compressed_size=$(stat -c %s "$archive")

    compression_percent=$(awk "BEGIN {
        if ($original_size == 0) print 0;
        else print ((($original_size - $compressed_size) / $original_size) * 100)
    }")

    compression_percent=$(printf "%.2f" "$compression_percent")

    echo "$file | $original_size | $compressed_size | $compression_percent %"
done
