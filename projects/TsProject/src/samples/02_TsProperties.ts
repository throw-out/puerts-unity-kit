export function init(target: CS.XOR.TsProperties) {
    let properties = target?.GetProperties();
    if (properties) {
        let stringBuilder: string[] = [];
        for (let i = 0; i < properties.Length; i++) {
            let property = properties.get_Item(i);
            stringBuilder.push(`${property.key}: ${property.value}`);
        }
        console.log(stringBuilder.join("\n"));
    } else {
        console.log("empty");
    }
}