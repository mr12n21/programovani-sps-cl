#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "binary.h"
#include "morse.h"
#include "huffman.h"

#define MAX_INPUT 2048

const char *AUTHOR = "Marek Broz";
const char *CLASS = "INF3";

void print_header() {
    printf("=========================================\n");
    printf("  Datové struktury v kódování\n");
    printf("  Autor: %s, třída: %s\n", AUTHOR, CLASS);
    printf("=========================================\n");
}

void read_line(char *buf, size_t size) {
    if (fgets(buf, (int)size, stdin)) {
        size_t len = strlen(buf);
        if (len > 0 && buf[len - 1] == '\n') buf[len - 1] = '\0';
    } else buf[0] = '\0';
}

int main(void) {
    char input[MAX_INPUT] = "";
    int choice;

    print_header();

    while (1) {
        printf("\nMenu:\n");
        printf("1) Zadat text\n");
        printf("2) Binární kódování (UNICODE styl)\n");
        printf("3) Morseova abeceda\n");
        printf("4) Huffmanovo kódování\n");
        printf("5) Konec\n");
        printf("Vyber (1-5): ");

        char opt[8];
        read_line(opt, sizeof(opt));
        choice = atoi(opt);

        switch (choice) {
            case 1:
                printf("Zadej text:\n");
                read_line(input, sizeof(input));
                break;
            case 2:
                if (*input) text_to_binary_unicode((unsigned char*)input);
                else printf("Zadej nejprve text!\n");
                break;
            case 3:
                if (*input) {
                    printf("-- Text -> Morse --\n");
                    text_to_morse(input);
                    printf("-- Morse -> Text --\n");
                    morse_to_text(input);
                } else printf("Zadej nejprve text!\n");
                break;
            case 4:
                if (*input) do_huffman((unsigned char*)input);
                else printf("Zadej nejprve text!\n");
                break;
            case 5:
                printf("Konec programu.\n");
                return 0;
            default:
                printf("Neplatná volba.\n");
        }
    }
}
