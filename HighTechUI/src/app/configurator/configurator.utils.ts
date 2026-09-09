export const RuleCodes = {
    COMPLETE_BUILD: 'COMPLETE_BUILD',
    ASSEMBLY_SERVICE: 'ASSEMBLY_SERVICE',
} as const;

const CATEGORY_ICONS: { [key: string]: string } = {
    'cpu': 'bi-cpu',
    'motherboard': 'bi-motherboard',
    'gpu': 'bi-display-fill',
    'ram': 'bi-memory',
    'storage': 'bi-hdd-fill',
    'case': 'bi-pc-display-horizontal',
    'psu': 'bi-lightning-charge-fill',
    'cpu cooler': 'bi-fan',
    'assembly': 'bi-tools'
};

export function getCategoryIcon(name: string): string {
    return CATEGORY_ICONS[(name ?? '').trim().toLowerCase()] ?? 'bi-box';
}
