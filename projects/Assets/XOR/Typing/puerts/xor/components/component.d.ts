declare class TsComponentImpl extends xor.TsBehaviour {
}
declare namespace TsComponentImpl {
    function guid(guid: number | string): ClassDecorator;
    function route(path: string): ClassDecorator;
    function fieldType(): PropertyDecorator;
}
declare global {
    namespace xor {
        class TsComponent extends TsComponentImpl {
        }
    }
}
export {};
