import os

ls=os.listdir()
num_lines=0

for it in ls:
    if (it[len(it)-1]=='s')and(it[len(it)-2]=='c'):
        num_lines += sum(1 for line in open(it))

print(num_lines)
