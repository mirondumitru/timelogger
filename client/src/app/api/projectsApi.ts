const BASE_URL = "http://localhost:3001/api";

export interface Sort {
    sortBy: string,
    sortOrder: string
}

export async function getAll(sort: Sort) {
    var url = `${BASE_URL}/projects`;

    if (sort) {
        url += "?";
    }

    if (sort.sortBy) {
        url += "sort.sortBy=" + sort.sortBy + "&";
    }

    if (sort.sortOrder) {
        url += "sort.sortOrder=" + sort.sortOrder;
    }

    const response = await fetch(url);
    return response.json();
}

export async function get(id: string) {
    const response = await fetch(`${BASE_URL}/projects/` + id);
    return response.json();
}

