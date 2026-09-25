import type { FC } from 'react';
import type { UseFormReturn } from 'react-hook-form';
import { LibrarySection } from '@/components/configForm/fields/librarySection.tsx';
import type { ConfigFormType } from '@/components/configForm/formSchema.tsx';
import {
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form.tsx';
import type { Library } from '@/types';

interface Props {
  form: UseFormReturn<ConfigFormType>;
  serverName: string;
  libraries: Library[];
}

export const LibrariesField: FC<Props> = ({ form, serverName, libraries }) => {
  return (
    <FormField
      control={form.control}
      name="libraries"
      render={() => (
        <FormItem className="rounded-lg border p-2">
          <div className="mb-4">
            <FormLabel className="text-base">Catalogs</FormLabel>
            <FormDescription>
              Select which Jellyfin libraries to include in Stremio discovery.
            </FormDescription>
          </div>
          {libraries.length > 0 ? (
            libraries.map((library) => (
              <LibrarySection
                key={library.key}
                form={form}
                serverName={serverName}
                library={library}
              />
            ))
          ) : (
            <div className="flex flex-col items-center justify-center">
              <div className="w-16 h-16 rounded-full animate-spin border-t-4 border-muted-foreground" />
              <span className="mt-4 text-lg text-muted-foreground text-center">
                No libs :(
              </span>
            </div>
          )}
          <FormMessage />
        </FormItem>
      )}
    />
  );
};
