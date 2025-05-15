export async function pigeonholeSort(arr, draw, sleep, stats = { comparisons: 0, swaps: 0 }) {
  let min = arr[0];
  let max = arr[0];
  for (let i = 1; i < arr.length; i++) {
    stats.comparisons += 2;
    if (arr[i] < min) min = arr[i];
    if (arr[i] > max) max = arr[i];
  }

  let range = max - min + 1;
  let pigeonhole = new Array(range);

  for (let i = 0; i < range; i++) {
    pigeonhole[i] = 0;
  }

  for (let i = 0; i < arr.length; i++) {
    pigeonhole[arr[i] - min]++;
    await draw(arr, [i], `array-container-${stats.containerIndex || 1}`);
    await sleep(50);
  }

  let index = 0;
  for (let i = 0; i < range; i++) {
    while (pigeonhole[i] > 0) {
      arr[index] = i + min;
      stats.swaps++;
      pigeonhole[i]--;
      await draw(arr, [index], `array-container-${stats.containerIndex || 1}`);
      await sleep(50);
      index++;
    }
  }
}






export async function testingPigeonholeSort(arr, draw, sleep, stats = { comparisons: 0, swaps: 0, containerIndex: 1 }) {
    let min = arr[0];
    let max = arr[0];
    for (let i = 1; i < arr.length; i++) {
      const val = arr[i];
      stats.comparisons++;
      if (val < min) min = val;
      stats.comparisons++
      if (val > max) max = val;
    }
  
    const range = max - min + 1;
  
    const pigeonhole = new Array(range);
    for (let i = 0; i < range; i++) pigeonhole[i] = 0;
  
    const drawInterval = Math.max(1, Math.floor(arr.length / 20));
    for (let i = 0; i < arr.length; i++) {
      pigeonhole[arr[i] - min]++;
      if (i % drawInterval === 0) { 
        await draw(arr, [i], `array-container-${stats.containerIndex}`);
        await sleep(50);
      }
    }
    await draw(arr, [], `array-container-${stats.containerIndex}`);
    await sleep(50);
  
    let index = 0;
    for (let i = 0; i < range; i++) {
      const count = pigeonhole[i];
      if (count > 0) {
        for (let j = 0; j < count; j++) {
          arr[index] = i + min;
          stats.swaps++;
          if (index % drawInterval === 0) {
            await draw(arr, [index], `array-container-${stats.containerIndex}`);
            await sleep(50);
          }
          index++;
        }
      }
    }
    await draw(arr, [], `array-container-${stats.containerIndex}`);
    await sleep(50);
  }