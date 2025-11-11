CC = gcc
CFLAGS = -Wall -Wextra -std=c11
OBJ = main.o binary.o morse.o huffman.o

all: kodovani

kodovani: $(OBJ)
	$(CC) $(CFLAGS) -o kodovani $(OBJ)

main.o: main.c binary.h morse.h huffman.h
binary.o: binary.c binary.h
morse.o: morse.c morse.h
huffman.o: huffman.c huffman.h

clean:
	rm -f *.o kodovani

run: kodovani
	./kodovani