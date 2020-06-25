# -*- coding: utf-8 -*-
"""
Created on Thu Jun 25 16:10:36 2020

@author: qtckp
"""


with open('README.md', 'r') as f:
    t = f.readlines()
    

with open('README.md', 'w') as f:
    for l in t:
        f.write(l.lstrip())
