/// <reference path="components/component.d.ts" />
export declare function get(obj: object, key: any): any;
export declare function set(obj: object, key: any, value: any): void;
export declare function getInPath(obj: object, key: string): any;
export declare function setInPath(obj: object, key: string, value: any): any;
export declare function containsKey(obj: object, key: any): boolean;
export declare function removeKey(obj: object, key: any): void;
export declare function getKeys(obj: object): import("csharp").System.Array$1<string>;
export declare function length(obj: object): number;
export declare function forEach(obj: object, action: CS.System.Delegate & ((key: any, value: any) => void)): void;
export declare function call(obj: object, methodName: string, args: CS.System.Array$1<any>): any;
