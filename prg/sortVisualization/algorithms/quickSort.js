export async function quickSort(arr, draw, sleep) {
    async function partition(low, high) {
      let pivot = arr[high];
      let i = low - 1;
      for (let j = low; j < high; j++) {
        if (arr[j] <= pivot) {
          i++;
          [arr[i], arr[j]] = [arr[j], arr[i]];
          await draw(arr, [i, j]);
          await sleep(50);
        }
      }
      [arr[i + 1], arr[high]] = [arr[high], arr[i + 1]];
      await draw(arr, [i + 1, high]);
      await sleep(50);
      return i + 1;
    }
  
    async function sort(low, high) {
      if (low < high) {
        const pi = await partition(low, high);
        await sort(low, pi - 1);
        await sort(pi + 1, high);
      }
    }
  
    await sort(0, arr.length - 1);
  }