export async function countingSort(arr, draw, sleep) {
  const max = Math.max(...arr);
  const min = Math.min(...arr);
  const range = max - min + 1;
  const count = new Array(range).fill(0);

  for (let i = 0; i < arr.length; i++) {
    count[arr[i] - min]++;
    await draw(arr, [i]);
    await sleep(10);
  }

  let index = 0;
  for (let i = 0; i < range; i++) {
    while (count[i] > 0) {
      arr[index] = i + min;
      await draw(arr, [index]);
      await sleep(10);
      index++;
      count[i]--;
    }
  }
}
