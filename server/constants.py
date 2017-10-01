unactivity_ttl = 5000 # 5 seconds

keys = ("alive",
        "id",
        "query",
        "position",
        "sort",
        "dest_point",
        "message",
        )

dict_s_i = {}
dict_i_s = {}

i = 0
for k in keys:
        dict_s_i[k] = i
        dict_i_s[i] = k
        i = i+1

