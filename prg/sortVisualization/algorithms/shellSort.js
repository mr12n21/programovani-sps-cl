export async function shellSort(arr, draw, sleep) {
    const n = arr.length;
    for (let gap = Math.floor(n / 2); gap > 0; gap = Math.floor(gap / 2)) {
      for (let i = gap; i < n; i++) {
        let temp = arr[i];
        let j;
        for (j = i; j >= gap && arr[j - gap] > temp; j -= gap) {
          arr[j] = arr[j - gap];
          await draw(arr, [j, j - gap]);
          await sleep(50);
        }
        arr[j] = temp;
        await draw(arr, [j]);
        await sleep(50);
      }
    }
  }