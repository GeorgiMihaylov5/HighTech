export interface AffectedField {
    fieldId: string;
    fieldName: string;
    productCount: number;
    sampleProducts: string[];
}

export interface CategoryRemovalPreview {
    totalAffectedProducts: number;
    affectedFields: AffectedField[];
    sampleProducts: string[];
}
