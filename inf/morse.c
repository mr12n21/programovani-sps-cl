#include <stdio.h>
#include <string.h>
#include <ctype.h>

void text_to_morse(char *text) {
    const char *morse_code[] = {
        ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", 
        "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-",
        "..-", "...-", ".--", "-..-", "-.--", "--..",
        "-----", ".----", "..---", "...--", "....-",
        ".....", "-....", "--...", "---..", "----."
    };

    for (int i = 0; text[i] != '\0'; i++) {
        char c = toupper(text[i]);
        if (c >= 'A' && c <= 'Z') {
            printf("%s ", morse_code[c - 'A']);
        } else if (c >= '0' && c <= '9') {
            printf("%s ", morse_code[c - '0' + 26]);
        } else if (c == ' ') {
            printf("/ ");
        }
    }
    printf("\n");
}

void morse_to_text(char *morse) {
    const char *morse_code[] = {
        ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", 
        "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-",
        "..-", "...-", ".--", "-..-", "-.--", "--..",
        "-----", ".----", "..---", "...--", "....-",
        ".....", "-....", "--...", "---..", "----."
    };

    char *token = strtok(morse, " ");
    while (token != NULL) {
        if (strcmp(token, "/") == 0) {
            printf(" ");
        } else {
            for (int i = 0; i < 36; i++) {
                if (strcmp(token, morse_code[i]) == 0) {
                    if (i < 26) {
                        printf("%c", 'A' + i);
                    } else {
                        printf("%c", '0' + (i - 26));
                    }
                    break;
                }
            }
        }
        token = strtok(NULL, " ");
    }
    printf("\n");
}

int main() {
    char text[100];
    printf("Zadej text: ");
    fgets(text, sizeof(text), stdin);

    text[strcspn(text, "\n")] = '\0';

    printf("Output: ");
    text_to_morse(text);
    morse_to_text(text);
    return 0;
}