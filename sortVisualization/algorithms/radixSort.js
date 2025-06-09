export async function radixSort(array, drawArray, sleep) {
  if (array.length === 0) return;

  const getMax = arr => Math.max(...arr);
  const maxNum = getMax(array);
  let exp = 1;

  while (Math.floor(maxNum / exp) > 0) {
    await countingSortByDigit(array, exp, drawArray, sleep);
    exp *= 10;
  }
}

async function countingSortByDigit(array, exp, drawArray, sleep) {
  const output = new Array(array.length).fill(0);
  const count = new Array(10).fill(0);

  for (let i = 0; i < array.length; i++) {
    const digit = Math.floor(array[i] / exp) % 10;
    count[digit]++;
    await drawArray(array, [i]);
    await sleep(10);
  }

  for (let i = 1; i < 10; i++) {
    count[i] += count[i - 1];
  }

  for (let i = array.length - 1; i >= 0; i--) {
    const digit = Math.floor(array[i] / exp) % 10;
    const pos = count[digit] - 1;
    output[pos] = array[i];
    count[digit]--;
  }

  for (let i = 0; i < array.length; i++) {
    array[i] = output[i];
    await drawArray(array, [i]);
    await sleep(10);
  }
}