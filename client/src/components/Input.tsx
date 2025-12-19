export default function Input({
                                  name,
                                  label,
                                  type = "text",
                                  defaultValue,
                                  required,
    className = ""
                              }: {
    name: string;
    label: string;
    type?: string;
    defaultValue?: string;
    required?: boolean;
    className?: string;
}) {
    return (
        <div>
            <label className="block mb-1 text-sm font-medium">{label}</label>
            <input
                name={name}
                type={type}
                defaultValue={defaultValue}
                required={required}
                className={`input input-bordered w-full ${className}`}
            />
        </div>
    );
}
