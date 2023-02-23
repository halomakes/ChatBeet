export function distinct<T>(value: T, index: number, array: T[]) {
    return array.indexOf(value) === index;
}