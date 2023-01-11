declare class TsComponentImpl extends xor.TsBehaviour {
}
declare namespace TsComponentImpl {
    function ComponentId(guid: number | string): ClassDecorator;
    function ComponentAlias(uniqueName: string): ClassDecorator;
}
declare global {
    namespace xor {
        class TsComponent extends TsComponentImpl {
        }
    }
}
export {};
