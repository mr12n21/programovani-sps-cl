export async function countingSort(arr, draw, sleep) {
  if (arr.length === 0) return;

  let min = arr[0];
  let max = arr[0];
  for (let i = 1; i < arr.length; i++) {
    if (arr[i] < min) min = arr[i];
    if (arr[i] > max) max = arr[i];
  }

  const range = max - min + 1;
  const count = new Array(range).fill(0);

  for (let i = 0; i < arr.length; i++) {
    count[arr[i] - min]++;
    await draw(arr, [i]);
    await sleep(15);
  }

  for (let i = 1; i < count.length; i++) {
    count[i] += count[i - 1];
    await sleep(5);
  }

  const output = new Array(arr.length);

  for (let i = arr.length - 1; i >= 0; i--) {
    const value = arr[i];
    const pos = count[value - min] - 1;
    output[pos] = value;
    count[value - min]--;
  }

  for (let i = 0; i < arr.length; i++) {
    arr[i] = output[i];
    await draw(arr, [i]);
    await sleep(15);
  }
}
