package com.tanjo3.runesandmasteries;

import java.util.List;

public class ListCount implements Comparable<ListCount> {

    private final int count;
    private final String list;

    public ListCount(List x) {
        count = x.size();
        list = x.toString();
    }

    public int getCount() {
        return count;
    }

    public String getList() {
        return list;
    }

    @Override
    public int compareTo(ListCount x) {
        if (count == x.getCount()) {
            return x.getList().compareTo(list);
        } else {
            return count - x.getCount();
        }
    }

    @Override
    public String toString() {
        return count + ": " + list;
    }
}
