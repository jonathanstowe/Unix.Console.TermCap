#
# $Id: Makefile,v 1.1.1.1 2004/09/07 20:14:11 jonathan Exp $
#

SRC = TermCap.cs \
		AssemblyInfo.cs

TARGET = Unix.Console.TermCap.dll

MCS = gmcs


$(TARGET):	$(SRC)
	$(MCS) /t:library -out:$(TARGET) -keyfile:Unix.Console.TermCap.snk $(SRC)

clean:
	rm -f *~ $(TARGET)


