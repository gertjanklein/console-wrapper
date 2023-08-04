import sys, io

import functools
print = functools.partial(print, flush=True)

def main():
    sys.stdout = io.TextIOWrapper(open(sys.stdout.fileno(), 'wb', 0), write_through=True)
    sys.stderr = io.TextIOWrapper(open(sys.stderr.fileno(), 'wb', 0), write_through=True)
    
    print("sys.argv:", " ".join(sys.argv))
    
    for i in range(5):
        print(f"stdout {i+1}")
        print(f"stderr {i+1}", file=sys.stderr)


if __name__ == '__main__':
    main()
    

