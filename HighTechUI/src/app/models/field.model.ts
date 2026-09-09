export interface Field {
    id: string;
    name: string;
    typeCode: TypeCode;
    value: any;
}

export enum TypeCode {
    Empty = 0,
    Object = 1,
    DBNull = 2,
    Boolean = 3,
    Char = 4,
    SByte = 5,
    Byte = 6,
    Int16 = 7,
    UInt16 = 8,
    Int32 = 9,
    UInt32 = 10,
    Int64 = 11,
    UInt64 = 12,
    Single = 13,
    Double = 14,
    Decimal = 15,
    DateTime = 16,
    String = 18
}

export interface TypeCodeOption {
    value: TypeCode;
    label: string;
}

export const UI_TYPE_CODE_OPTIONS: TypeCodeOption[] = [
    { value: TypeCode.String, label: 'Text' },
    { value: TypeCode.Int32, label: 'Integer' },
    { value: TypeCode.Decimal, label: 'Decimal' },
];

export function getTypeCodeLabel(typeCode: TypeCode | number | null | undefined): string {
    if (typeCode === null || typeCode === undefined) {
        return '';
    }

    const curated = UI_TYPE_CODE_OPTIONS.find(o => o.value === typeCode);
    if (curated) {
        return curated.label;
    }

    return TypeCode[typeCode] ?? String(typeCode);
}