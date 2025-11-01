#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include "huffman.h"

#define MAX_NODES 512
#define MAX_CODE_LEN 256

typedef struct Node {
    unsigned char ch;
    unsigned int freq;
    struct Node *left, *right;
} Node;

typedef struct MinHeap {
    Node *nodes[MAX_NODES];
    int size;
} MinHeap;

static Node* new_node(unsigned char ch, unsigned int freq) {
    Node *n = malloc(sizeof(Node));
    n->ch = ch;
    n->freq = freq;
    n->left = NULL;
    n->right = NULL;
    return n;
}

static void heap_swap(Node **a, Node **b) {
    Node *t = *a;
    *a = *b;
    *b = t;
}

static void heap_push(MinHeap *h, Node *n) {
    int i = h->size++;
    h->nodes[i] = n;

    while (i > 0) {
        int parent = (i - 1) / 2;
        if (h->nodes[parent]->freq <= h->nodes[i]->freq)
            break;

        heap_swap(&h->nodes[parent], &h->nodes[i]);
        i = parent;
    }
}

static Node* heap_pop(MinHeap *h) {
    if (h->size == 0)
        return NULL;

    Node *root = h->nodes[0];
    h->nodes[0] = h->nodes[--h->size];
    int i = 0;
    while (1) {
        int left = 2 * i + 1;
        int right = 2 * i + 2;
        int smallest = i;

        if (left < h->size && h->nodes[left]->freq < h->nodes[smallest]->freq)
            smallest = left;
        if (right < h->size && h->nodes[right]->freq < h->nodes[smallest]->freq)
            smallest = right;

        if (smallest == i)
            break;

        heap_swap(&h->nodes[i], &h->nodes[smallest]);
        i = smallest;
    }

    return root;
}

static Node* build_huffman(unsigned int freq[256]) {
    MinHeap heap = { .size = 0 };

    for (int i = 0; i < 256; i++) {
        if (freq[i])
            heap_push(&heap, new_node(i, freq[i]));
    }

    if (heap.size == 0)
        return NULL;
    while (heap.size > 1) {
        Node *a = heap_pop(&heap);
        Node *b = heap_pop(&heap);

        Node *parent = new_node(0, a->freq + b->freq);
        parent->left = a;
        parent->right = b;

        heap_push(&heap, parent);
    }

    return heap_pop(&heap);
}


static void generate_codes(Node *root, char codes[256][MAX_CODE_LEN],
                           char *buffer, int depth) {
    if (!root)
        return;
    if (!root->left && !root->right) {
        buffer[depth] = '\0';
        strcpy(codes[root->ch], buffer);
        return;
    }

    buffer[depth] = '0';
    generate_codes(root->left, codes, buffer, depth + 1);

    buffer[depth] = '1';
    generate_codes(root->right, codes, buffer, depth + 1);

}

static void print_tree(Node *root, int level) {
    if (!root)
        return;

    for (int i = 0; i < level; i++)
        putchar(' ');

    if (!root->left && !root->right) {
        if (isprint(root->ch))
            printf("'%c' (%u)\n", root->ch, root->freq);
        else
            printf("0x%02X (%u)\n", root->ch, root->freq);
    } else {
        printf("* (%u)\n", root->freq);
    }

    print_tree(root->left, level + 2);
    print_tree(root->right, level + 2);
}

static void free_tree(Node *root) {
    if (!root)
        return;
    free_tree(root->left);
    free_tree(root->right);
    free(root);
}


void do_huffman(const unsigned char *text) {
    unsigned int freq[256] = {0};

    for (size_t i = 0; text[i]; i++)
        freq[text[i]]++;

    Node *root = build_huffman(freq);
    if (!root) {
        printf("Prazdny vstup.\n");
        return;
    }
    printf("\n=== Cetnosti znaku ===\n");
    for (int i = 0; i < 256; i++) {
        if (freq[i])
            printf("%c (%u)\n", isprint(i) ? i : '?', freq[i]);
    }

    printf("\n=== Huffmanuv strom ===\n");
    print_tree(root, 0);

    char codes[256][MAX_CODE_LEN] = {0};
    char buffer[MAX_CODE_LEN];
    generate_codes(root, codes, buffer, 0);

    printf("\n=== Huffmanovy kody ===\n");
    for (int i = 0; i < 256; i++) {
        if (freq[i])
            printf("%c : %s\n", isprint(i) ? i : '?', codes[i]);
    }

    printf("\n=== Zakodovany text ===\n");
    for (size_t i = 0; text[i]; i++)
        printf("%s", codes[text[i]]);

    printf("\n");

    free_tree(root);
}
