#include <stdio.h>
#include <string.h>
#include <ctype.h>
#include "morse.h"

const char *MORSE_CODE[] = {
    ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---",
    "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-",
    "..-", "...-", ".--", "-..-", "-.--", "--..",
    "-----", ".----", "..---", "...--", "....-",
    ".....", "-....", "--...", "---..", "----."
};

void text_to_morse(const char *text) {
    printf("Morse:\n");
    for (int i = 0; text[i] != '\0'; i++) {
        char c = toupper((unsigned char)text[i]);
        if (c >= 'A' && c <= 'Z') {
            fputs(MORSE_CODE[c - 'A'], stdout);
            putchar(' ');
        } else if (c >= '0' && c <= '9') {
            fputs(MORSE_CODE[c - '0' + 26], stdout);
            putchar(' ');
        } else if (isspace((unsigned char)c)) {
            putchar('/');
            putchar(' ');
        } else {
            printf("? ");
        }
    }
    putchar('\n');
}

char morse_to_char(const char *code) {
    for (size_t i = 0; i < sizeof(MORSE_CODE)/sizeof(MORSE_CODE[0]); i++) {
        if (strcmp(code, MORSE_CODE[i]) == 0) {
            if (i < 26) return 'A' + i;
            else return '0' + (i - 26);
        }
    }
    return '?';
}

void morse_to_text(const char *morse) {
    char buf[16];
    size_t b = 0;
    printf("Morse -> Text:\n");
    for (size_t i = 0; ; i++) {
        char ch = morse[i];
        if (ch == ' ' || ch == '\0') {
            if (b == 1 && buf[0] == '/') {
                putchar(' ');
            } else if (b > 0) {
                buf[b] = '\0';
                putchar(morse_to_char(buf));
            }
            b = 0;
            if (ch == '\0') break;
        } else {
            if (b < sizeof(buf)-1) buf[b++] = ch;
        }
    }
    putchar('\n');
}
