
/**非funtion键值 */
type NonFunctionKeys<T extends object> = { [K in keyof T]?: T[K] extends Function ? never : K }[keyof T];

type NonFunction<T extends object> = Pick<T, NonFunctionKeys<T>>;

/**判断两个类型是否签名一致 */
type Equals<X, Y, A = X, B = never> =
    (<T>() => T extends X ? 1 : 2) extends
    (<T>() => T extends Y ? 1 : 2) ? A : B;

type ConstructorType<T> = { new(...args: any[]): T };

/**获取类型成员的类型 */
type MemberType<T, TKey extends keyof T> = T[TKey];
/**获取function第一个参数类型 */
type FirstParameter<T extends Function> = T extends (param1: infer R, ...args: any[]) => void ? R : never;
type SecondParameter<T extends Function> = T extends (param1: any, param2: infer R, ...args: any[]) => void ? R : never;
type ThirdParameter<T extends Function> = T extends (param1: any, param2: any, param3: infer R, ...args: any[]) => void ? R : never;

/**获取Array成员类型 */
type ArrayMemberType<T> = T extends Array<infer R> ? R : never;

/**获取Promise成员类型 */
type PromiseMemberType<T> = T extends Promise<infer R> ? R : never;
