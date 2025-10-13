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

int main() {
    char text[100];
    printf("Zadej text: ");
    fgets(text, sizeof(text), stdin);

    text[strcspn(text, "\n")] = '\0';

    printf("Morse: ");
    text_to_morse(text);
    return 0;
}