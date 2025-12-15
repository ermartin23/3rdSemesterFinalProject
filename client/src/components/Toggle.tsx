export default function Toggle({
                                   name,
                                   label,
                                   defaultChecked
                               }: {
    name: string;
    label: string;
    defaultChecked?: boolean;
}) {
    return (
        <label className="flex items-center gap-2">
            <input
                type="checkbox"
                name={name}
                className="toggle"
                defaultChecked={defaultChecked}
            />
            <span>{label}</span>
        </label>
    );
}
