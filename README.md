# Console wrapper

**WIP!**

This program is intended to wrap a console program, and display its
stdout and stderr streams in a text box. Ultimate goal is to add a few
buttons with additional functionality.

Due to buffering issues, stderr and stdout can't be sparately handled;
they would not be displayed in order. Therefore, the console program is
called with stderr redirected to stdout.
