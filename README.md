Unix.Console.TermCap
====================

Unix termcap library for Mono (c#)

It should be sufficient to simpy run

    make

By default it uses the v2.0 compiler, if this isn't suitable
then edit the Makefile replacing the gmcs with mcs.

There is no install target in the makefile, if you want to use
this from the global assembly cache you can use gacutil

   gacutil -i Unix.Console.TermCap.dll

probably with root permissions.
