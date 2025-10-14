#include <stdio.h>
#include <string.h>
#include <stdlib.h>

void char_to_binary(int ascii, char *binary) {
    for (int i = 7; i >= 0; i--) {
        binary[7 - i] = (ascii & (1 << i)) ? '1' : '0';
    }
    binary[8] = '\0'; 
}

void text_to_ascii_to_binary(char *text, char *output) {
    char binary[9];
    output[0] = '\0';

    for (int i = 0; text[i] != '\0'; i++) {
        char_to_binary(text[i], binary);
        char line[50];
        snprintf(line, sizeof(line), "1: '%c' | 2: %d | 3: %s\n", text[i], text[i], binary);
        strcat(output, line);
    }
}

int main() {
    char input[1000];
    char output[10000] = "";

    printf("Zadej text: ");
    if (fgets(input, sizeof(input), stdin)) {

        size_t len = strlen(input);
        if (len > 0 && input[len - 1] == '\n') {
            input[len - 1] = '\0';
            len--;
        }

        if (len == 0) {
            printf("neni vstup\n");
            return 0;
        }

        text_to_ascii_to_binary(input, output);
        printf("\nVysledek:\n%s", output);
    } else {
        printf("neni vstup\n");
    }

    return 0;
}