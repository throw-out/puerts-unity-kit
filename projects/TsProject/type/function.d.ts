type NonFunctionKeys<T extends object> = { [K in keyof T]?: T[K] extends Function ? never : K }[keyof T];
type NonFunction<T extends object> = Pick<T, NonFunctionKeys<T>>;
type ConstructorType<T> = { new(...args: any[]): T };