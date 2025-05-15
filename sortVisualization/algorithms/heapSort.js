export async function heapSort(arr, draw, sleep) {
    async function heapify(n, i) {
      let largest = i;
      const left = 2 * i + 1;
      const right = 2 * i + 2;
  
      if (left < n && arr[left] > arr[largest]) {
        largest = left;
      }
      if (right < n && arr[right] > arr[largest]) {
        largest = right;
      }
  
      if (largest !== i) {
        [arr[i], arr[largest]] = [arr[largest], arr[i]];
        await draw(arr, [i, largest]);
        await sleep(50);
        await heapify(n, largest);
      }
    }
  
    // Build max heap
    for (let i = Math.floor(arr.length / 2) - 1; i >= 0; i--) {
      await heapify(arr.length, i);
    }
  
    // Extract elements from heap
    for (let i = arr.length - 1; i > 0; i--) {
      [arr[0], arr[i]] = [arr[i], arr[0]];
      await draw(arr, [0, i]);
      await sleep(50);
      await heapify(i, 0);
    }
  }