export class FormDataUtils {
    static toFormData(obj: any, form?: FormData, namespace?: string): FormData {
        const formData = form ?? new FormData();

        Object.keys(obj).forEach(key => {
            const value = obj[key];
            if(value === null || value === undefined)
                return;

            const formKey = namespace ? `${namespace}.${key}` : key;

            if(value instanceof Date)
                formData.append(formKey, value.toISOString());
            else if (value instanceof File || value instanceof Blob)
                formData.append(formKey, value);
            else if (Array.isArray(value))
                value.forEach((item, index) => {
                    if(item instanceof File || item instanceof Blob)
                        formData.append(`${formKey}[${index}]`, item)
                    else if (typeof item === 'object')
                        this.toFormData(item, formData, `${formKey}[${index}]`);
                    else
                        formData.append(`${formKey}[${index}]`, item.toString());
                });
            else if (typeof value === 'object')
                this.toFormData(value, formData, formKey);
            else
                formData.append(formKey, value.toString());
        });

        return formData;
    }
}