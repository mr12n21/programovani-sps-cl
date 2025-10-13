#include <stdio.h>
#include <string.h>
#include <ctype.h>

#define ARRAY_SIZE(x) (sizeof(x) / sizeof(x[0]))

const char *MORSE_CODE[] = {
    ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---",
    "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-",
    "..-", "...-", ".--", "-..-", "-.--", "--..",
    "-----", ".----", "..---", "...--", "....-",
    ".....", "-....", "--...", "---..", "----."
};

void text_to_morse(const char *text) {
    for (int i = 0; text[i] != '\0'; i++) {
        char c = toupper((unsigned char)text[i]);
        if (c >= 'A' && c <= 'Z') {
            printf("%s ", MORSE_CODE[c - 'A']);
        } else if (c >= '0' && c <= '9') {
            printf("%s ", MORSE_CODE[c - '0' + 26]);
        } else if (isspace(c)) {
            printf("/ ");
        }
    }
    printf("\n");
}

char morse_to_char(const char *code) {
    for (int i = 0; i < ARRAY_SIZE(MORSE_CODE); i++) {
        if (strcmp(code, MORSE_CODE[i]) == 0) {
            return (i < 26) ? ('A' + i) : ('0' + (i - 26));
        }
    }
    return '?';
}

void morse_to_text(const char *morse) {
    char buffer[8];
    int j = 0;

    for (int i = 0; ; i++) {
        if (morse[i] == ' ' || morse[i] == '\0') {
            buffer[j] = '\0';
            if (strcmp(buffer, "/") == 0) {
                printf(" ");
            } else if (j > 0) {
                printf("%c", morse_to_char(buffer));
            }
            j = 0;

            if (morse[i] == '\0')
                break;
        } else {
            buffer[j++] = morse[i];
        }
    }
    printf("\n");
}

int main() {
    char input[256];
    int volba;

    printf("Imput: ");
    fgets(input, sizeof(input), stdin);
    input[strcspn(input, "\n")] = '\0';

    printf("Output: ");
    text_to_morse(input); morse_to_text(input);
    return 0;
}
