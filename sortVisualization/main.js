import { drawArray, sleep, shuffleArray, generateArray } from './visualizer/renderer.js';
import { getSelectedAlgorithm } from './visualizer/controls.js';
import { quickSort } from './algorithms/quickSort.js';
import { heapSort } from './algorithms/heapSort.js';
import { shellSort } from './algorithms/shellSort.js';
import { pigeonholeSort } from './algorithms/pigeonholeSort.js';
import { countingSort } from './algorithms/countingSort.js';

const arraySize = 50;
let array = generateArray(arraySize);

window.addEventListener('load', () => {
  console.log('Page loaded, drawing initial array');
  drawArray(array);
});

document.getElementById('shuffle-button').addEventListener('click', () => {
  console.log('Shuffling array');
  array = shuffleArray([...array]);
  drawArray(array);
});

document.getElementById('start-button').addEventListener('click', async () => {
  console.log('Starting sort with algorithm:', getSelectedAlgorithm());
  const algorithm = getSelectedAlgorithm();
  const arrayCopy = [...array];
  switch (algorithm) {
    case 'quickSort':
      await quickSort(arrayCopy, drawArray, sleep);
      break;
    case 'heapSort':
      await heapSort(arrayCopy, drawArray, sleep);
      break;
    case 'shellSort':
      await shellSort(arrayCopy, drawArray, sleep);
      break;
    case 'pigeonholeSort':
      await pigeonholeSort(arrayCopy, drawArray, sleep);
      break;
    case 'countingSort':
      await countingSort(arrayCopy, drawArray, sleep);
      break;
  }
  array = arrayCopy;
  drawArray(array);
});