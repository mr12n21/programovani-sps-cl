function radixSortObjects(arr, key) {
  if (arr.length === 0) return arr;

  let max = arr[0][key];
  for (let i = 1; i < arr.length; i++) {
    if (arr[i][key] > max) max = arr[i][key];
  }

  let exp = 1;
  const n = arr.length;
  let output = new Array(n);

  while (max / exp >= 1) {
    const count = new Array(10).fill(0);

    for (let i = 0; i < n; i++) {
      const digit = Math.floor(arr[i][key] / exp) % 10;
      count[digit]++;
    }

    for (let i = 1; i < 10; i++) {
      count[i] += count[i - 1];
    }

    for (let i = n - 1; i >= 0; i--) {
      const digit = Math.floor(arr[i][key] / exp) % 10;
      output[--count[digit]] = arr[i];
    }

    for (let i = 0; i < n; i++) {
      arr[i] = output[i];
    }

    exp *= 10;
  }
  return arr;
}

function testRadixSortObjects() {
  const SIZE = 10_000_000;
  const arr = new Array(SIZE);
  for (let i = 0; i < SIZE; i++) {
    arr[i] = { value: (Math.random() * 1e9) | 0, index: i };
  }

  console.time('RadixSortObjects');
  radixSortObjects(arr, 'value');
  console.timeEnd('RadixSortObjects');

  // for (let i = 1; i < SIZE; i++) {
  //   if (arr[i - 1].value > arr[i].value) {
  //     console.log('Chyba v řazení na indexu', i);
  //     break;
  //   }
  // }
}

testRadixSortObjects();