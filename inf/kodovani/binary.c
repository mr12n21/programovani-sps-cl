#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include <stdint.h>
#include "binary.h"

void char_to_binary16(uint16_t code, char *binary) {
    for (int i = 15; i >= 0; i--) {
        binary[15 - i] = (code & (1 << i)) ? '1' : '0';
    }
    binary[16] = '\0';
}

void text_to_binary_unicode(const unsigned char *text) {
    printf("binarni:\n");
    char bin[17];
    for (size_t i = 0; i < strlen((const char*)text); i++) {
        uint16_t val = (uint16_t) text[i];
        char display = isprint(text[i]) ? text[i] : '?';
        char_to_binary16(val, bin);
        printf("1: '%c' | 2: %u | 3: %s\n", display, (unsigned int)val, bin);
    }
    putchar('\n');
}
