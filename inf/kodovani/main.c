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
    printf("  Datove struktury v kodovani\n");
    printf("  Autor: %s, trida: %s\n", AUTHOR, CLASS);
    printf("=========================================\n");
}

void read_line(char *buf, size_t size) {
    if (fgets(buf, (int)size, stdin)) {
        size_t len = strlen(buf);
        if (len > 0 && buf[len - 1] == '\n') buf[len - 1] = '\0';
    } else {
        buf[0] = '\0';
    }
}

int ensure_input(char *input, size_t size) {
    if (input[0] != '\0') return 1;

    printf("zadat text (y/n): ");
    char opt[8];
    read_line(opt, sizeof(opt));
    if (opt[0] == 'y' || opt[0] == 'Y') {
        printf("Zadej text:\n");
        read_line(input, size);
        if (input[0] != '\0') return 1;
        printf("error\n");
        return 0;
    }
    return 0;
}

int main(void) {
    char input[MAX_INPUT] = "";
    int choice;

    print_header();

    while (1) {
        printf("\nMenu:\n");
        printf("1) Zadat text\n");
        printf("2) Binarni kodovani\n");
        printf("3) Morseova abeceda\n");
        printf("4) Huffmanovo kodovani\n");
        printf("5) Konec\n");
        printf("Vyber: ");

        char opt[8];
        read_line(opt, sizeof(opt));
        choice = atoi(opt);

        switch (choice) {
            case 1:
                printf("Zadej text:\n");
                read_line(input, sizeof(input));
                break;

            case 2:
                if (!ensure_input(input, sizeof(input))) {
                    printf("Akce zrušena. Nejprve zadej text.\n");
                } else {
                    text_to_binary_unicode((unsigned char*)input);
                }
                break;

            case 3:
                if (!ensure_input(input, sizeof(input))) {
                    printf("error\n");
                } else {
                    printf("-- Text -> Morse --\n");
                    text_to_morse(input);
                    printf("\n-- Morse -> Text --\n");
                    morse_to_text(input);
                }
                break;

            case 4:
                if (!ensure_input(input, sizeof(input))) {
                    printf("error\n");
                } else {
                    do_huffman((unsigned char*)input);
                }
                break;

            case 5:
                printf("end\n");
                return 0;

            default:
                printf("error\n");
        }
    }
}